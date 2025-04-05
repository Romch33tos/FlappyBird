import tkinter as tk
import random
from PIL import Image, ImageTk
import shutil

#shutil.unpack_archive("Python/Программы/Игры/Flappy Bird/game_files.zip", "Python/Программы/Игры/Flappy Bird/game_files")

def delete():
    #shutil.rmtree("Python/Программы/Игры/Flappy Bird/game_files")
    root.destroy()

class FlappyBird:
    def __init__(self, master):
        self.master = master
        self.master.title("Flappy Bird")
        self.master.resizable(False, False)
        root.protocol("WM_DELETE_WINDOW", delete)
        self.pipe_image = Image.open("game_files/p1.png").resize((50, 400))
        self.pipe_image = ImageTk.PhotoImage(self.pipe_image)

        self.bird_image = Image.open("game_files/b.png").resize((50, 50))
        self.bird_image = ImageTk.PhotoImage(self.bird_image)

        self.width = 300
        self.height = 400
        self.gravity = 0.5
        self.jump_strength = 8.5

        self.canvas = tk.Canvas(master, width=self.width, height=self.height, bg="skyblue")
        self.canvas.pack()

        self.score = 0
        self.game_over = False
        self.bird = None
        self.pipes = []
        self.game_started = False
        self.start_message = None
        self.score_text = None
        self.update_id = None

        self.master.bind("<ButtonPress-1>", self.on_click)
        self.show_start_message()

    def show_start_message(self):
        self.start_message = self.canvas.create_text(
            self.width / 2, self.height / 2, text="Кликни, чтобы\n  начать игру!", font=("Arial", 20), fill="black"
        )

    def show_game_over_message(self):
        self.canvas.create_text(
            self.width / 2, self.height / 2, text="Конец игры!", font=("Arial", 20), fill="black"
        )

    def start_game(self):
        self.score = 0
        self.game_over = False
        self.bird_y_velocity = 0
        self.pipes.clear()
        self.canvas.delete("all")

        self.canvas.delete(self.start_message)
        self.bird = self.canvas.create_image(50, self.height / 2, anchor=tk.NW, image=self.bird_image)
        self.score_text = self.canvas.create_text(45, 15, text=f"Счёт: {self.score}", font=("Arial", 16), fill="black")
        self.create_pipe()

        if self.update_id is not None:
            self.master.after_cancel(self.update_id)

        self.update()

    def create_pipe(self):
        gap_start = random.randint(100, self.height - 200)
        top_pipe = self.canvas.create_image(self.width, gap_start - 400, anchor=tk.NW, image=self.pipe_image)
        bottom_pipe = self.canvas.create_image(self.width, gap_start + 150, anchor=tk.NW, image=self.pipe_image)
        self.pipes.append((top_pipe, bottom_pipe))

    def on_click(self, event):
        if not self.game_started:
            self.game_started = True
            self.start_game()
        elif self.game_over:
            self.start_game()
        else:
            self.flap()

    def flap(self):
        self.bird_y_velocity = -self.jump_strength

    def update(self):
        if not self.game_over:
            self.bird_y_velocity += self.gravity
            self.canvas.move(self.bird, 0, self.bird_y_velocity)

            for top_pipe, bottom_pipe in self.pipes:
                self.canvas.move(top_pipe, -3, 0)
                self.canvas.move(bottom_pipe, -3, 0)

            if self.pipes and self.canvas.coords(self.pipes[0][0])[0] < -50:
                top_pipe, bottom_pipe = self.pipes.pop(0)
                self.canvas.delete(top_pipe)
                self.canvas.delete(bottom_pipe)
                self.score += 1 

            if len(self.pipes) == 0 or self.canvas.coords(self.pipes[-1][0])[0] < self.width - 200:
                self.create_pipe()

            if self.check_collision():
                self.game_over = True
                self.show_game_over_message()

            self.canvas.itemconfig(self.score_text, text=f"Счёт: {self.score}")
            self.canvas.tag_raise(self.score_text)

            self.update_id = self.master.after(15, self.update)

    def check_collision(self):
        bird_coords = self.canvas.coords(self.bird)
        bird_x, bird_y = bird_coords[0], bird_coords[1]
        bird_width, bird_height = 50, 50

        for top_pipe, bottom_pipe in self.pipes:
            top_coords = self.canvas.coords(top_pipe)
            bottom_coords = self.canvas.coords(bottom_pipe)

            if (bird_x + bird_width > top_coords[0] and bird_x < top_coords[0] + 50 and
                bird_y < top_coords[1] + 400):
                return True

            if (bird_x + bird_width > bottom_coords[0] and bird_x < bottom_coords[0] + 50 and
                bird_y + bird_height > bottom_coords[1]):
                return True

        if bird_y < 0 or bird_y + bird_height > self.height:
            return True

        return False

if __name__ == "__main__":
    root = tk.Tk()
    game = FlappyBird(root)
    root.mainloop()
    
