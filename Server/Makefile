CC=g++
FLAGS=-g -Wall
LDOPT=

all: server

clean:
	rm -rf *.o
	rm -rf server

cbase.o: cbase.cpp cbase.h
	$(CC) -c $(FLAGS) -o cbase.o cbase.cpp

cconnection.o: cconnection.cpp cconnection.h
	$(CC) -c $(FLAGS) -o cconnection.o cconnection.cpp

cserver.o: cserver.cpp cserver.h
	$(CC) -c $(FLAGS) -o cserver.o cserver.cpp

portable_socket.o: portable_socket.cpp portable_socket.h
	$(CC) -c $(FLAGS) -o portable_socket.o portable_socket.cpp

server.o: Server.cpp
	$(CC) -c $(FLAGS) -o Server.o Server.cpp

#server: server.o cbase.o cconnection.o cserver.o
#	$(CC) -o server cbase.o cconnection.o cserver.o Server.o $(LDOPT)

server: server.o portable_socket.o
	$(CC) -o server portable_socket.o Server.o $(LDOPT)
