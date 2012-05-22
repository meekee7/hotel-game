#!/bin/bash

###############
# About: Auto restart Hotel server on crash & generate crash log
###############

# config:
# path to server binary
daemon=./hotel_server
# system
export LD_LIBRARY_PATH=.:lib:$LD_LIBRARY_PATH

if [ "`ulimit -c`" -eq 0 ]; then
   ulimit -c unlimited
fi

while true
do 
   SERVER=`ps -el | grep hotel_server`
   $daemon $*
   if [ -z "$SERVER" ]; then
      echo "El servidor ha caido o se ha cerrado, tienes 3 segundos para darle a Control + C"
      sleep 3
   fi
   sleep 2
done
