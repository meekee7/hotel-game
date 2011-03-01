CC=g++
FLAGS=-g -Wall
LDOPT= -lpthread -lnsl

all: server

clean:
	rm -rf *.o
	rm -rf server

player.o: player.cpp player.h
	$(CC) -c $(FLAGS) -o player.o player.cpp

game.o: game.cpp game.h
	$(CC) -c $(FLAGS) -o game.o game.cpp

portable_socket.o: portable_socket.cpp portable_socket.h
	$(CC) -c $(FLAGS) -o portable_socket.o portable_socket.cpp

server.o: Server.cpp
	$(CC) -c $(FLAGS) -o Server.o Server.cpp

server: portable_socket.o game.o player.o server.o
	$(CC) -o server portable_socket.o game.o player.o Server.o dlib/all/source.cpp $(LDOPT) -DDLIB_NO_GUI_SUPPORT
