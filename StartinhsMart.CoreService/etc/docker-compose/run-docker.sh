#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p d7747d85-11a3-4344-b85e-8c5c744aca69 -t
    fi
    cd ../
fi

docker-compose up -d
