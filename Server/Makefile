CC=g++
FLAGS=-g -Wall
LDOPT= -lpthread -lnsl -lmysqlclient

all: Server

clean:
	rm -rf *.o
	rm -rf hotel_server

Aux_Functions.o: Aux_Functions.cpp Aux_Functions.h
	$(CC) -c $(FLAGS) -o Aux_Functions.o Aux_Functions.cpp

Random.o: Random.cpp Random.h
	$(CC) -c $(FLAGS) -o Random.o Random.cpp

Chat.o: Chat.cpp Chat.h
	$(CC) -c $(FLAGS) -o Chat.o Chat.cpp

Player.o: Player.cpp Player.h
	$(CC) -c $(FLAGS) -o Player.o Player.cpp

PlayerGameState.o: PlayerGameState.cpp PlayerGameState.h
	$(CC) -c $(FLAGS) -o PlayerGameState.o PlayerGameState.cpp

Game.o: Game.cpp Game.h
	$(CC) -c $(FLAGS) -o Game.o Game.cpp

Position.o: Position.cpp Position.h
	$(CC) -c $(FLAGS) -o Position.o Position.cpp

Hotel.o: Hotel.cpp Hotel.h
	$(CC) -c $(FLAGS) -o Hotel.o Hotel.cpp

Portable_Socket.o: Portable_Socket.cpp Portable_Socket.h
	$(CC) -c $(FLAGS) -o Portable_Socket.o Portable_Socket.cpp

TinyXML.o: TinyXML.cpp TinyXML.h
	$(CC) -c $(FLAGS) -o TinyXML.o TinyXML.cpp

TinyStr.o: TinyStr.cpp TinyStr.h
	$(CC) -c $(FLAGS) -o TinyStr.o TinyStr.cpp

TinyXMLError.o: TinyXMLError.cpp
	$(CC) -c $(FLAGS) -o TinyXMLError.o TinyXMLError.cpp

TinyXMLParser.o: TinyXMLParser.cpp
	$(CC) -c $(FLAGS) -o TinyXMLParser.o TinyXMLParser.cpp

Server.o: Server.cpp
	$(CC) -c $(FLAGS) -o Server.o Server.cpp
	
MySQLMgr.o: MySQLMgr.cpp MySQLMgr.h
	$(CC) -c $(FLAGS) -o MySQLMgr.o MySQLMgr.cpp

SavedgamesMgr.o: SavedgamesMgr.cpp SavedgamesMgr.h
	$(CC) -c $(FLAGS) -o SavedgamesMgr.o SavedgamesMgr.cpp

Server: Portable_Socket.o TinyXMLError.o TinyXMLParser.o TinyStr.o TinyXML.o Chat.o Game.o Position.o Hotel.o Player.o PlayerGameState.o Server.o Random.o Aux_Functions.o MySQLMgr.o SavedgamesMgr.o
	$(CC) -o hotel_server Portable_Socket.o TinyXMLError.o TinyXMLParser.o TinyStr.o TinyXML.o Chat.o Game.o Hotel.o Player.o PlayerGameState.o Position.o Random.o Aux_Functions.o MySQLMgr.o SavedgamesMgr.o Server.o dlib/all/source.cpp $(LDOPT) -DDLIB_NO_GUI_SUPPORT
