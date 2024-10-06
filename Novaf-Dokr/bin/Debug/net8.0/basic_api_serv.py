from flask import Flask, jsonify, request, abort
import sys
import logging
import subprocess
from functools import wraps

app = Flask(__name__)

node_name = None
auth_username = None
auth_password = None
auth_token = None  # Variable to store the generated auth token
token_validity = 30  # Validity in seconds
shutdown_called = False  # Track if shutdown has been called

# Set up logging to output errors
logging.basicConfig(level=logging.DEBUG)

# Basic auth decorator
def check_auth(username, password):
    return username == auth_username and password == auth_password

def requires_auth(f):
    @wraps(f)
    def decorated(*args, **kwargs):
        auth = request.headers.get('Authorization')
        if auth != f"Bearer {auth_token}":
            logging.warning("Authorization failed!")
            return jsonify({'message': 'Authorization required!'}), 401
        return f(*args, **kwargs)
    return decorated

@app.route('/')
def index():
    return f"API Server for {node_name} is running!"

@app.route('/hello', methods=['GET'])
def hello():
    return jsonify({'message': f'Greetings from {node_name}, running on {request.host}'}), 200

@app.route('/routes', methods=['GET'])
def list_routes():
    routes = [rule.rule for rule in app.url_map.iter_rules()]
    return jsonify({'routes': routes}), 200

@app.route('/f/auth/login', methods=['POST'])
def login():
    username = request.json.get('username')
    password = request.json.get('password')
    
    if check_auth(username, password):
        global auth_token
        auth_token = f"{username}-token"  # Simple token generation
        logging.info("Login successful, token generated.")
        return jsonify({'auth-token': auth_token}), 200
    else:
        logging.warning("Login failed: invalid credentials.")
        return jsonify({'message': 'Invalid username or password!'}), 401

@app.route('/f/system/shutdown', methods=['POST'])
@requires_auth
def shutdown():
    try:
        exit()
        return jsonify({'message': 'Shutting down the server...'})
    except Exception as e:
        return jsonify({"message": "Error occurred during shutdown!", "error": str(e)}), 500

def shutdown_server():
    func = request.environ.get('werkzeug.server.shutdown')
    if func is None:
        logging.error("Shutdown function not available.")
        exit()
        raise RuntimeError('Not running with the Werkzeug Server')
    func()

@app.route('/f/ssh/cmd', methods=['POST'])
@requires_auth
def run_command():
    command = request.json.get('command')
    if not command:
        return jsonify({'message': 'No command provided!'}), 400
    
    try:
        # Run the command and capture the output
        result = subprocess.run(command, shell=True, capture_output=True, text=True)
        response = {
            'stdout': result.stdout.strip(),
            'stderr': result.stderr.strip(),
            'return_code': result.returncode
        }
        return jsonify({'response': response}), 200
    except Exception as e:
        logging.error(f"Error running command: {e}")
        return jsonify({'message': 'Error running command!'}), 500

if __name__ == '__main__':
    if len(sys.argv) != 5:
        print("Usage: python basic_api_serv.py <node_name> <auth_username> <auth_password> <port>")
        sys.exit(1)

    node_name = sys.argv[1]
    auth_username = sys.argv[2]
    auth_password = sys.argv[3]
    port = int(sys.argv[4])

    logging.info(f"Starting server for node: {node_name} on port: {port}")
    app.run(host='0.0.0.0', port=port)
