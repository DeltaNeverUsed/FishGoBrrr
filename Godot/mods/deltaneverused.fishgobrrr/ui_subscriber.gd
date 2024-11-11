extends Node

var main: Node

func _ready():
	main = get_parent()
	get_tree().root.connect("child_entered_tree", self, "_on_child_entered_tree")

func _on_child_entered_tree(child: Node):
	if not child.is_connected("child_entered_tree", self, "_on_child_entered_tree"):
		child.connect("child_entered_tree", self, "_on_child_entered_tree")
	
	if child is Button:
		if not child.is_connected("pressed", self, "_on_button_pressed"):
			child.connect("pressed", self, "_on_button_pressed")
		if not child.is_connected("mouse_entered", self, "_on_mouse_entered"):
			child.connect("mouse_entered", self, "_on_mouse_entered")

func _on_button_pressed():
	main.click()

func _on_mouse_entered():
	main.hover()
