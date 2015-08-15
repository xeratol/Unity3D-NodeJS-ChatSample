var net = require('net');

var clients = [];

var s = net.Server(function (socket) {
	console.log("Client connected");
	for (var i in clients) {
		clients[i].write("user joined");
	}

	clients.push(socket);

	socket.on('data', function (msg) {
		console.log("" + msg); // force msg to be a string

		// This is where you put logic and parse your different message types

		// For now, just send the message to everyone
		for (var i in clients) {
			clients[i].write(msg);
		}
	});

	socket.on('end', function () {
		console.log("Client disconnected");
		var i = clients.indexOf(socket);
		clients.splice(i, 1);

		for (var i in clients) {
			clients[i].write("user left");
		}
	});
});

s.listen(8000);

//* Handle ctrl+c event
process.on('SIGINT', function () {
	for (var i in clients) {
		clients[i].write("Server shutdown");
		clients[i].destroy();
	}

	console.log('Exiting properly');
	process.exit(0);
});
//*/

console.log("Chat Server listening " + s.address().address + ":" + s.address().port);