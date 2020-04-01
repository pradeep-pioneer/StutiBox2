# StutiBox2

## Setting static IP address

Update the DHCP daemon config file by editing the file ```/etc/dhcpcd.conf```. Add the following lines to the bottom of the file (obviously check that these don't exist already in uncommented state).

```bash
interface wlan0
static ip_address=192.168.1.192/24
static ip6_address=fd51:42f8:caae:d92e::ff/64
static routers=192.168.1.1
static domain_name_servers=192.168.1.1 8.8.8.8 fd51:42f8:caae:d92e::1
```

Save the file and run ```reboot``` from the terminal. Verify the IP address by running ```ifconfig``` after restart.

## Copy the id file to pi for ssh commands

```bash
ssh-copy-id  -i [filename:example ~/.ssh/id_pi_rsa] [pi_username@pi_ip_address]
```

## To Do Items

1. Add option to configure Raspberry Pi display.
2. Migrate the volume settings to UnoSquare library from BASS based calls.
3. Add screen to configure alarms.
4. Add alarm configuration storage to support persistance of settings. [In Progress]
5. Add Systems Configuration Panel to manage configurations.
6. ~~Add auto restart to make sure that the system is fresh [Done].~~
