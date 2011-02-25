Brutalo Deluxe Network Engine v. 1.27
by Frank Drebin

Features:
- windows/linux c++ socket wrapper using tcp protocol
- provides non-blocking functions using select()
- automatic buffer management
- automatic id management
- supports up to 64 connections (1 server + 63 clients)
- single packet can be up to 255 bytes long
- designed for client-server model applications

Notes:
- MAX_CLIENTS in cserver.h defines the maximum number of clients (up to 63)
- MAX_BUFFER in cclient.h and cconnection.h defines the maximum buffer size
- in windows link with -lwsock32 to compile
