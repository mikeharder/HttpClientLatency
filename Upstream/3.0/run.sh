#!/usr/bin/env bash

docker run -it --rm --network host "$@" upstream:3.0
