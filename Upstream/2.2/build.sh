#!/usr/bin/env bash

docker build -t upstream:2.2 -f ./Dockerfile .. "$@"
