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
      gdb $daemon core --batch --eval-command="bt ful" > crash.log
      dte=`date +%F_%H-%M-%S`
      mkdir torta_$dte
      mv crash.log torta_$dte/
      rm -rf core
      echo "Log de caida generado, tienes otros 3 segundos para darle a Control + C"
      sleep 3
      killall -s 9 hotel_server
   fi
   sleep 2
done
