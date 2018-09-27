#!/usr/bin/env bash

docker build -t upstream:2.1 -f ./Dockerfile .. "$@"
