#!/bin/bash
address=${1:-pi@192.168.1.192}
echo "**************************************************************"
echo "Started deployment on $address "
echo "**************************************************************"
# remove the artificats locally
echo "remove the artificats locally"
rm -rf bin/release/

echo "building client package..."
yarn
yarn build:dev

echo "restoring packages ..."
dotnet restore
echo "publishing ..."
dotnet publish -c release -r linux-arm

# remove the remote artifacts
echo "deploying to: $address ..."
echo "stopping service on $address"
ssh -i ~/.ssh/id_pi_rsa $address "sudo service StutiBox2 stop"
echo "attempting to disable the service on $address"
ssh -i ~/.ssh/id_pi_rsa $address "sudo systemctl disable StutiBox2.service"
echo "attempting to remove the service unit file on $address"
ssh -i ~/.ssh/id_pi_rsa $address "sudo rm /lib/systemd/system/StutiBox2.service"

echo "removing files from $address ..."
ssh -i ~/.ssh/id_pi_rsa $address "rm ~/StutiBox/ -rf"

echo "copying published files to $address ..."
scp -i ~/.ssh/id_pi_rsa -r bin/release/netcoreapp3.1/linux-arm/publish/ $address:~/StutiBox/

echo "Copying the service files to designated location on $address"
ssh -i ~/.ssh/id_pi_rsa $address sudo cp "/home/pi/StutiBox/StutiBox2.service" "/lib/systemd/system/StutiBox2.service"

echo "Enabling the service on $address"
ssh -i ~/.ssh/id_pi_rsa $address sudo systemctl daemon-reload
ssh -i ~/.ssh/id_pi_rsa $address sudo systemctl enable StutiBox2.service
echo "Starting the service on $address"
ssh -i ~/.ssh/id_pi_rsa $address sudo systemctl start StutiBox2.service
echo "**************************************************************"
echo "Finished deployment on $address "
echo "**************************************************************"