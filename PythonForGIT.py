import socket
import time
import pandas as pd

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

# Set up a DataFrame to store and display the data
columns = ['PosX', 'PosY', 'PosZ', 'RotX', 'RotY', 'RotZ']
data_table = pd.DataFrame(columns=columns)

try:
    while True:
        time.sleep(0.5)  # Sleep to simulate real-time communication

        # Example control signal (throttle in this case)
        control_signal = '1.0'  # Example throttle value
        sock.sendall(control_signal.encode("UTF-8"))

        # Receiving updated position and rotation from Unity
        received_data = sock.recv(1024).decode("UTF-8").strip()
        print("Received:", received_data)

        # Splitting position and rotation data
        pos_data, rot_data = received_data.split(';')
        position = list(map(float, pos_data.split(',')))
        rotation = list(map(float, rot_data.split(',')))

        # Append the new row to the DataFrame
        new_row = pd.DataFrame([position + rotation], columns=columns)
        data_table = pd.concat([data_table, new_row], ignore_index=True)

        # Clear the output and display updated DataFrame
        print(data_table.tail())  # Display the last few rows to keep output manageable

except KeyboardInterrupt:
    sock.close()
    print("Socket closed")
    print(data_table)  # Optionally print the entire table at the end
