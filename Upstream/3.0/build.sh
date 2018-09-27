#!/usr/bin/env bash

docker build -t upstream:3.0 -f ./Dockerfile .. "$@"
