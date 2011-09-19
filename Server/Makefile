CC=g++
FLAGS=-g -Wall
LDOPT= -lpthread -lnsl

all: server

clean:
	rm -rf *.o
	rm -rf server

chat.o: chat.cpp chat.h
	$(CC) -c $(FLAGS) -o chat.o chat.cpp

player.o: player.cpp player.h
	$(CC) -c $(FLAGS) -o player.o player.cpp

game.o: game.cpp game.h
	$(CC) -c $(FLAGS) -o game.o game.cpp

portable_socket.o: portable_socket.cpp portable_socket.h
	$(CC) -c $(FLAGS) -o portable_socket.o portable_socket.cpp

Server.o: Server.cpp
	$(CC) -c $(FLAGS) -o Server.o Server.cpp

server: portable_socket.o chat.o game.o player.o Server.o
	$(CC) -o server portable_socket.o chat.o game.o player.o Server.o dlib/all/source.cpp $(LDOPT) -DDLIB_NO_GUI_SUPPORT
