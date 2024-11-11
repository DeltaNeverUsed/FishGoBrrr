extends Node

var tcp_client := StreamPeerTCP.new()

enum Action {
	Vibrate,
	StopAll,
	Hover,
	Click
}

var continues = 0
var last_continues = 0

var last_vib = 0
var last_vib_dur = 0
var last_vib_time = Time.get_ticks_msec();

func connect_to_server(host: String, port: int):
	var connection_status = tcp_client.connect_to_host(host, port)
	if connection_status == OK:
		print("Connected to server")
		return true
	else:
		print("Failed to connect: ", connection_status)
		return false

func add_script_node(path: String):
	var node = Node2D.new()
	node.set_script(load(path))
	add_child(node)

func _ready():
	add_script_node("res://mods/deltaneverused.fishgobrrr/ui_subscriber.gd")
	
	connect_to_server("127.0.0.1", 7345)
	vibrate(0.5, 0.1)

func _physics_process(delta):
	if (continues > 0.02):
		if (last_vib_time + last_vib_dur - Time.get_ticks_msec() <= 0):
			vibrate(continues, 0)
		else:
			vibrate(continues + last_vib, 0)
	elif last_continues > 0.02 && last_vib_time + last_vib_dur - Time.get_ticks_msec() <= 0:
		stop()
	last_continues = continues
	continues = lerp(continues, 0, 0.1)

func send_request(data):
	var data_to_send = JSON.print(data).to_utf8()
	var bytes_sent = tcp_client.put_data(data_to_send)

func stop():
	send_request({
		"Action": Action.StopAll,
		"Value": 0,
		"Duration": -1
	})

func continues_vibrate(power):
	continues += power

func vibrate(power, duration):
	if (power < 0):
		return
	last_vib = power
	last_vib_dur = duration * 1000
	last_vib_time = Time.get_ticks_msec();
	send_request({
		"Action": Action.Vibrate,
		"Value": power,
		"Duration": duration
	})

func hover():
	send_request({
		"Action": Action.Hover,
		"Value": 0,
		"Duration": -1
	})

func click():
	send_request({
		"Action": Action.Click,
		"Value": 0,
		"Duration": -1
	})
