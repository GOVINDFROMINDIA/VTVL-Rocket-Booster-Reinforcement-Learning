import socket
import time

def main():
    host, port = "127.0.0.1", 25001
    position = [0, 0, 0]  # Initial position
    rotation = [0, 0, 0]  # Initial rotation

    while True:
        try:
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
                sock.connect((host, port))
                for _ in range(100):  # Send data 100 times or until an error occurs
                    time.sleep(0.5)
                    position[0] += 1  # Increment X position by one

                    # Format the position and rotation data
                    data_string = f"{','.join(map(str, position))};{','.join(map(str, rotation))}"
                    print(data_string)
                    sock.sendall(data_string.encode("UTF-8"))

                    received_data = sock.recv(1024).decode("UTF-8")
                    print(received_data)

                break  # Exit the loop after sending data 100 times

        except ConnectionError as e:
            print(f"Connection error: {e}, retrying in 5 seconds...")
            time.sleep(5)  # Wait for 5 seconds before retrying
        except Exception as e:
            print(f"An error occurred: {e}")
            break  # Break the loop on other exceptions

if __name__ == "__main__":
    main()
