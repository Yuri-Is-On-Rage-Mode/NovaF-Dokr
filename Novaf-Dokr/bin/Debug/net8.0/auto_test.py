import requests
import json
import time

# Set the API base URL
BASE_URL = "http://127.0.0.1:7002"  # Change this if needed

# User credentials
USERNAME = "user333"
PASSWORD = "password333"

# Test the /f/auth/login endpoint
def test_login():
    print("Testing /f/auth/login...")
    response = requests.post(
        f"{BASE_URL}/f/auth/login",
        json={"username": USERNAME, "password": PASSWORD},
    )
    if response.status_code == 200:
        auth_token = response.json().get("auth-token")
        print(f"Login successful! Auth token: {auth_token}")
        return auth_token
    else:
        print(f"Login failed: {response.status_code} - {response.text}")
        return None

# Test the /hello endpoint
def test_hello():
    print("Testing /hello...")
    response = requests.get(f"{BASE_URL}/hello")
    if response.status_code == 200:
        print(f"Response: {response.json()}")
    else:
        print(f"Failed: {response.status_code} - {response.text}")

# Test the /routes endpoint
def test_routes():
    print("Testing /routes...")
    response = requests.get(f"{BASE_URL}/routes")
    if response.status_code == 200:
        print(f"Available routes: {response.json()}")
    else:
        print(f"Failed: {response.status_code} - {response.text}")

# Test the /f/system/shutdown endpoint
def test_shutdown(auth_token):
    print("Testing /f/system/shutdown...")
    response = requests.post(
        f"{BASE_URL}/f/system/shutdown",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    if response.status_code == 200:
        print(f"Shutdown response: {response.json()}")
    else:
        print(f"Failed: {response.status_code} - {response.text}")

# Test the /f/ssh/cmd endpoint
def test_run_command(auth_token):
    print("Testing /f/ssh/cmd...")
    command = "dir"
    response = requests.post(
        f"{BASE_URL}/f/ssh/cmd",
        headers={"Authorization": f"Bearer {auth_token}"},
        json={"command": command},
    )
    if response.status_code == 200:
        print(f"Command response: {response.json()}")
    else:
        print(f"Failed: {response.status_code} - {response.text}")

def main():
    # Start by testing login to get the auth token
    auth_token = test_login()
    if not auth_token:
        print("Could not proceed without a valid auth token.")
        return

    # Test all other features
    test_hello()
    test_routes()
    
    # Introduce a delay to allow for server processing
    time.sleep(1)

    # Test shutdown (ensure to run this last, as it will shut down the server)
    test_shutdown(auth_token)

    # To demonstrate running a command, you may want to test it before shutdown
    time.sleep(1)
    test_run_command(auth_token)

if __name__ == "__main__":
    main()
