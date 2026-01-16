@echo off
setlocal enabledelayedexpansion

echo =====================================
echo AWS ECS Fargate Deployment Script
echo =====================================
echo.

REM Configuration
set "PROJECT_NAME=minimal-api-project"
set "TASK_FAMILY=!PROJECT_NAME!-task"
set "SERVICE_NAME=!PROJECT_NAME!-service"

REM Prompt for AWS configuration
echo --- AWS Configuration ---
set /p "AWS_REGION=Enter AWS region (e.g., us-east-1): "
set "AWS_DEFAULT_REGION=!AWS_REGION!"

echo.
echo --- ECS Cluster Configuration ---
set /p "CLUSTER_NAME=Enter ECS cluster name (e.g., my-ecs-cluster): "

REM Check if cluster exists
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECS cluster
        exit /b 1
    )
)

echo.
echo --- Network Configuration ---
set /p "VPC_ID=Enter VPC ID (e.g., vpc-0abc123def456): "
set /p "SUBNET_IDS=Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p "SECURITY_GROUP=Enter Security Group ID (e.g., sg-0abc123def): "

REM Parse subnet IDs
for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set "SUBNET_1=%%a"
    set "SUBNET_2=%%b"
)
if "!SUBNET_2!"==" " set "SUBNET_2=!SUBNET_1!"

echo.
echo --- Docker Image Configuration ---
set /p "IMAGE_URI=Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): "

echo.
echo --- Database Configuration ---
set /p "DB_HOST=Enter database host (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): "
set /p "DB_NAME=Enter database name [minimalapi]: "
if "!DB_NAME!"==" " set "DB_NAME=minimalapi"
set /p "DB_USER=Enter database user [dbuser]: "
if "!DB_USER!"==" " set "DB_USER=dbuser"
set /p "DB_PASSWORD=Enter database password: "

REM Get AWS Account ID
echo.
echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set "ACCOUNT_ID=%%i"
echo Account ID: !ACCOUNT_ID!

REM Ask about load balancer
echo.
set /p "NEED_LB=Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo --- Creating Application Load Balancer ---
    
    set "ALB_NAME=!PROJECT_NAME!-alb"
    echo Creating Application Load Balancer...
    
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name !ALB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text') do set "ALB_ARN=%%i"
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create Application Load Balancer
        exit /b 1
    )
    
    echo ALB created: !ALB_ARN!
    
    REM Get ALB DNS
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !ALB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text') do set "ALB_DNS=%%i"
    
    echo Creating Target Group...
    set "TG_NAME=!PROJECT_NAME!-tg"
    
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path "/health" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text') do set "TARGET_GROUP_ARN=%%i"
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create Target Group
        exit /b 1
    )
    
    echo Target Group created: !TARGET_GROUP_ARN!
    
    echo Creating ALB Listener...
    aws elbv2 create-listener --load-balancer-arn !ALB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! >nul
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ALB Listener
        exit /b 1
    )
    
    echo ALB Listener created successfully
    echo.
) else (
    echo Skipping load balancer creation.
    set "TARGET_GROUP_ARN="
)

REM Create temporary copies
echo.
echo Preparing ECS task definition...
copy ecs\task-definition.json %TEMP%\task-definition.json >nul
copy ecs\service-definition.json %TEMP%\service-definition.json >nul

REM Replace placeholders using PowerShell
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_HOST}}', '!DB_HOST!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_NAME}}', '!DB_NAME!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_USER}}', '!DB_USER!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_PASSWORD}}', '!DB_PASSWORD!' | Set-Content %TEMP%\task-definition.json"

powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content %TEMP%\service-definition.json"

if "!TARGET_GROUP_ARN!"==" " (
    echo Removing load balancer configuration...
    powershell -Command "$json = Get-Content %TEMP%\service-definition.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content %TEMP%\service-definition.json"
) else (
    powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content %TEMP%\service-definition.json"
)

REM Register task definition
echo.
echo Registering ECS task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://%TEMP%\task-definition.json --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set "TASK_DEF_ARN=%%i"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to register task definition
    exit /b 1
)

echo Task definition registered: !TASK_DEF_ARN!

REM Check if service exists
echo.
echo Checking if ECS service exists...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[?status=='ACTIVE'].serviceName" --output text') do set "EXISTING_SERVICE=%%i"

if "!EXISTING_SERVICE!"==" " (
    echo Service does not exist. Creating new ECS service...
    aws ecs create-service --cluster !CLUSTER_NAME! --service-name !SERVICE_NAME! --cli-input-json file://%TEMP%\service-definition.json --region !AWS_REGION! >nul
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECS service
        exit /b 1
    )
    
    echo ECS service created successfully
) else (
    echo Service exists. Updating ECS service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --region !AWS_REGION! >nul
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to update ECS service
        exit /b 1
    )
    
    echo ECS service updated successfully
)

REM Wait for stability
echo.
echo Waiting for service to become stable (this may take a few minutes)...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

if !ERRORLEVEL! neq 0 (
    echo WARNING: Service did not stabilize within timeout
    echo Check ECS console for service status
) else (
    echo Service is stable
)

REM Deployment summary
echo.
echo =====================================
echo Deployment Summary
echo =====================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!
echo Region: !AWS_REGION!

if not "!TARGET_GROUP_ARN!"==" " (
    echo Load Balancer DNS: !ALB_DNS!
    echo Application URL: http://!ALB_DNS!
)

echo CloudWatch Logs: /ecs/!PROJECT_NAME!
echo.

for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].runningCount" --output text') do set "RUNNING_TASKS=%%i"

echo Running tasks: !RUNNING_TASKS!
echo.
echo =====================================
echo Deployment completed successfully!
echo =====================================
echo.
echo Troubleshooting tips:
echo 1. View service events: aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo 2. View task logs: aws logs tail /ecs/!PROJECT_NAME! --follow --region !AWS_REGION!
echo 3. List running tasks: aws ecs list-tasks --cluster !CLUSTER_NAME! --service-name !SERVICE_NAME! --region !AWS_REGION!
echo.

REM Cleanup
del %TEMP%\task-definition.json
del %TEMP%\service-definition.json

endlocal