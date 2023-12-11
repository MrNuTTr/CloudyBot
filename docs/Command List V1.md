# Command List (WIP)
### /setup
Start the setup wizard. This gives the user instructions on how to setup an Azure (or other) account and give the bot the required permissions. Should ask for default settings like region, delete public IPs, max online time, etc. The bot should do as much here as possible.

Eventually add support for more than Microsoft Azure.

### /add
Start the add server wizard. This should poll all virtual machines in the resource group that aren't being managed. This allows for users to create their own servers but let the bot turn them on/off.

### /server-list
List all current game servers and their status.

### /start <server_name>
Start the server with given <server_name>.

### /stop <server_name>
Stop the server with given <server_name>.

### /stop-all
Stop all turned on servers.

### /delete <server_name>
Stop and delete the server with given <server_name> and its associated IP.

### /delete-all
Stop and delete all servers and associated IPs.