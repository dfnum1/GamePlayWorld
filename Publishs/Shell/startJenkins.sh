#!/bin/bash

echo "start jenkins"
# java -jar /Applications/Jenkins/jenkins.war
brew services start jenkins
# Restart the Jenkins service: brew services restart jenkins
# Update the Jenkins version: brew upgrade jenkins
echo "end"
