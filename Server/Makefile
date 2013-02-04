CC=g++
FLAGS=-g -Wall
LDOPT= -lpthread -lnsl -lmysqlclient

all: server

clean:
	rm -rf *.o
	rm -rf hotel_server

aux_functions.o: aux_functions.cpp aux_functions.h
	$(CC) -c $(FLAGS) -o aux_functions.o aux_functions.cpp

random.o: random.cpp random.h
	$(CC) -c $(FLAGS) -o random.o random.cpp

chat.o: chat.cpp chat.h
	$(CC) -c $(FLAGS) -o chat.o chat.cpp

player.o: player.cpp player.h
	$(CC) -c $(FLAGS) -o player.o player.cpp

playergamestate.o: playergamestate.cpp playergamestate.h
	$(CC) -c $(FLAGS) -o playergamestate.o playergamestate.cpp

game.o: game.cpp game.h
	$(CC) -c $(FLAGS) -o game.o game.cpp

position.o: position.cpp position.h
	$(CC) -c $(FLAGS) -o position.o position.cpp

hotel.o: hotel.cpp hotel.h
	$(CC) -c $(FLAGS) -o hotel.o hotel.cpp

portable_socket.o: portable_socket.cpp portable_socket.h
	$(CC) -c $(FLAGS) -o portable_socket.o portable_socket.cpp

tinyxml.o: tinyxml.cpp tinyxml.h
	$(CC) -c $(FLAGS) -o tinyxml.o tinyxml.cpp

tinystr.o: tinystr.cpp tinystr.h
	$(CC) -c $(FLAGS) -o tinystr.o tinystr.cpp

tinyxmlerror.o: tinyxmlerror.cpp
	$(CC) -c $(FLAGS) -o tinyxmlerror.o tinyxmlerror.cpp

tinyxmlparser.o: tinyxmlparser.cpp
	$(CC) -c $(FLAGS) -o tinyxmlparser.o tinyxmlparser.cpp

server.o: server.cpp
	$(CC) -c $(FLAGS) -o server.o server.cpp
	
mysqlmgr.o: mysqlmgr.cpp mysqlmgr.h
	$(CC) -c $(FLAGS) -o mysqlmgr.o mysqlmgr.cpp

savedgamesmgr.o: savedgamesmgr.cpp savedgamesmgr.h
	$(CC) -c $(FLAGS) -o savedgamesmgr.o savedgamesmgr.cpp

server: portable_socket.o tinyxmlerror.o tinyxmlparser.o tinystr.o tinyxml.o chat.o game.o position.o hotel.o player.o playergamestate.o server.o random.o aux_functions.o mysqlmgr.o savedgamesmgr.o
	$(CC) -o hotel_server portable_socket.o tinyxmlerror.o tinyxmlparser.o tinystr.o tinyxml.o chat.o game.o hotel.o player.o playergamestate.o position.o random.o aux_functions.o mysqlmgr.o savedgamesmgr.o server.o dlib/all/source.cpp $(LDOPT) -DDLIB_NO_GUI_SUPPORT
