from flask import Flask, jsonify, request
import sys

app = Flask(__name__)

node_name = None

@app.route('/')
def index():
    return f"API Server for {node_name} is running!"

@app.route('/shutdown', methods=['POST'])
def shutdown():
    shutdown_server()
    return jsonify({'message': 'Shutting down the server...'})

def shutdown_server():
    func = request.environ.get('werkzeug.server.shutdown')
    if func is None:
        raise RuntimeError('Not running with the Werkzeug Server')
    func()

if __name__ == '__main__':
    if len(sys.argv) != 2:
        print("Usage: python basic_api_serv.py <node_name>")
        sys.exit(1)

    node_name = sys.argv[1]
    app.run(host='0.0.0.0', port=5000)
