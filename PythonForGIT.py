import socket
import time
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

positions = []  # Store positions for plotting
rotations = []  # Store rotations for plotting

try:
    while True:
        time.sleep(0.5)  # Sleep to simulate real-time communication

        # Example control signal (throttle in this case)
        control_signal = '1.0'  # Example throttle value
        sock.sendall(control_signal.encode("UTF-8"))

        # Receiving updated position and rotation from Unity
        received_data = sock.recv(1024).decode("UTF-8")
        print("Received:", received_data)

        # Splitting position and rotation data
        pos_data, rot_data = received_data.split(';')
        position_tuple = tuple(map(float, pos_data.split(',')))
        rotation_tuple = tuple(map(float, rot_data.split(',')))

        # Storing the received positions and rotations for plotting
        positions.append(position_tuple)
        rotations.append(rotation_tuple)

except KeyboardInterrupt:
    sock.close()
    print("Socket closed")

    # Plotting the trajectory
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    
    # Extracting x, y, z coordinates for positions
    x, y, z = zip(*positions)
    ax.plot(x, y, z, label='Trajectory')
    
    # Plotting start and end points
    ax.scatter(x[0], y[0], z[0], color='green', label='Start')
    ax.scatter(x[-1], y[-1], z[-1], color='red', label='End')
    
    ax.set_xlabel('X Position')
    ax.set_ylabel('Y Position')
    ax.set_zlabel('Z Position')
    ax.legend()
    
    plt.show()

