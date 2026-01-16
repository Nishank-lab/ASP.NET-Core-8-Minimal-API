#!/bin/bash
set -e
set -o pipefail

echo "====================================="
echo "AWS ECS Fargate Deployment Script"
echo "====================================="
echo ""

# Configuration
PROJECT_NAME="minimal-api-project"
TASK_FAMILY="${PROJECT_NAME}-task"
SERVICE_NAME="${PROJECT_NAME}-service"

# Prompt for AWS configuration
echo "--- AWS Configuration ---"
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
export AWS_DEFAULT_REGION="$AWS_REGION"

echo ""
echo "--- ECS Cluster Configuration ---"
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME

# Check if cluster exists, create if not
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create ECS cluster"
        exit 1
    fi
}

echo ""
echo "--- Network Configuration ---"
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Convert comma-separated subnets to array
IFS=',' read -ra SUBNETS <<< "$SUBNET_IDS"
SUBNET_1="${SUBNETS[0]}"
SUBNET_2="${SUBNETS[1]:-$SUBNET_1}"

echo ""
echo "--- Docker Image Configuration ---"
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): " IMAGE_URI

echo ""
echo "--- Database Configuration ---"
read -p "Enter database host (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): " DB_HOST
read -p "Enter database name [minimalapi]: " DB_NAME
DB_NAME=${DB_NAME:-minimalapi}
read -p "Enter database user [dbuser]: " DB_USER
DB_USER=${DB_USER:-dbuser}
read -sp "Enter database password: " DB_PASSWORD
echo ""

# Get AWS Account ID
echo ""
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"

# Ask about load balancer
echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "--- Creating Application Load Balancer ---"
    
    # Create ALB
    echo "Creating Application Load Balancer..."
    ALB_NAME="${PROJECT_NAME}-alb"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$ALB_NAME" \
        --subnets ${SUBNETS[@]} \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --tags Key=Name,Value="$ALB_NAME" Key=Project,Value="$PROJECT_NAME" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text)
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create Application Load Balancer"
        exit 1
    fi
    
    echo "ALB created: $ALB_ARN"
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    # Create Target Group with target-type ip (required for Fargate)
    echo "Creating Target Group..."
    TG_NAME="${PROJECT_NAME}-tg"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path "/health" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text)
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create Target Group"
        exit 1
    fi
    
    echo "Target Group created: $TARGET_GROUP_ARN"
    
    # Create Listener
    echo "Creating ALB Listener..."
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" >/dev/null
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create ALB Listener"
        exit 1
    fi
    
    echo "ALB Listener created successfully"
    echo ""
else
    echo "Skipping load balancer creation."
    TARGET_GROUP_ARN=""
fi

# Create temporary copies of JSON files
echo ""
echo "Preparing ECS task definition..."
cp ecs/task-definition.json /tmp/task-definition.json
cp ecs/service-definition.json /tmp/service-definition.json

# Replace placeholders in task definition
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" /tmp/task-definition.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" /tmp/task-definition.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" /tmp/task-definition.json
sed -i "s|{{DB_HOST}}|$DB_HOST|g" /tmp/task-definition.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" /tmp/task-definition.json
sed -i "s|{{DB_USER}}|$DB_USER|g" /tmp/task-definition.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" /tmp/task-definition.json

# Replace placeholders in service definition
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" /tmp/service-definition.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" /tmp/service-definition.json

if [ -z "$TARGET_GROUP_ARN" ]; then
    # Remove loadBalancers section if no LB
    echo "Removing load balancer configuration from service definition..."
    jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' /tmp/service-definition.json > /tmp/service-definition-no-lb.json
    mv /tmp/service-definition-no-lb.json /tmp/service-definition.json
else
    # Replace target group ARN
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" /tmp/service-definition.json
fi

# Register task definition
echo ""
echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file:///tmp/task-definition.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to register task definition"
    exit 1
fi

echo "Task definition registered: $TASK_DEF_ARN"

# Check if service exists
echo ""
echo "Checking if ECS service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text)

if [ -z "$EXISTING_SERVICE" ]; then
    echo "Service does not exist. Creating new ECS service..."
    aws ecs create-service \
        --cluster "$CLUSTER_NAME" \
        --service-name "$SERVICE_NAME" \
        --cli-input-json file:///tmp/service-definition.json \
        --region "$AWS_REGION" >/dev/null
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create ECS service"
        exit 1
    fi
    
    echo "ECS service created successfully"
else
    echo "Service exists. Updating ECS service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --force-new-deployment \
        --region "$AWS_REGION" >/dev/null
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to update ECS service"
        exit 1
    fi
    
    echo "ECS service updated successfully"
fi

# Wait for service to stabilize
echo ""
echo "Waiting for service to become stable (this may take a few minutes)..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

if [ $? -ne 0 ]; then
    echo "WARNING: Service did not stabilize within the timeout period"
    echo "Check the ECS console for service status"
else
    echo "Service is stable"
fi

# Verify deployment
echo ""
echo "====================================="
echo "Deployment Summary"
echo "====================================="
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"
echo "Region: $AWS_REGION"

if [ -n "$TARGET_GROUP_ARN" ]; then
    echo "Load Balancer DNS: $ALB_DNS"
    echo "Application URL: http://$ALB_DNS"
fi

echo "CloudWatch Logs: /ecs/$PROJECT_NAME"
echo ""

# Get running tasks count
RUNNING_TASKS=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

echo "Running tasks: $RUNNING_TASKS"
echo ""
echo "====================================="
echo "Deployment completed successfully!"
echo "====================================="
echo ""
echo "Troubleshooting tips:"
echo "1. View service events: aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo "2. View task logs: aws logs tail /ecs/$PROJECT_NAME --follow --region $AWS_REGION"
echo "3. List running tasks: aws ecs list-tasks --cluster $CLUSTER_NAME --service-name $SERVICE_NAME --region $AWS_REGION"
echo ""

# Cleanup
rm -f /tmp/task-definition.json /tmp/service-definition.json