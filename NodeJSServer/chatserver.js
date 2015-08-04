var net = require('net');

var clients = [];

var s = net.Server(function (socket) {
	console.log("Client connected");
	clients.push(socket);

	socket.on('data', function (msg) {
		console.log("Receive " + msg);

		for (var i in clients) {
			clients[i].write(msg);
		}
	});

	socket.on('end', function () {
		console.log("Client disconnected");
		var i = clients.indexOf(socket);
		clients.splice(i, 1);
	});
});

s.listen(8000);

/* Handle ctrl+c event
process.on('SIGINT', function () {
	for (var i in clients)
		clients[i].destroy();

	console.log('Exiting properly');
	process.exit(0);
});
*/

console.log("Chat Server listening " + s.address().address + ":" + s.address().port);