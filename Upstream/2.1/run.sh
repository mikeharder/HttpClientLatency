#!/usr/bin/env bash

docker run -it --rm --network host "$@" upstream:2.1
