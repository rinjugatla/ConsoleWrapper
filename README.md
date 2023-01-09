# ConsoleWrapper

## CommandSetting.json

コマンド定義サンプル

```json
[
    {
        "app": {
            "name": "bedrock_server.exe"
        },
        "basic_commands": [
            {
                "name": "show help",
                "description": "",
                "commands": [
                    {
                        "type": "console",
                        "command": "? <int>",
                        "pattern": "^\\? (?<page>1-3)$"
                    }
                ]
            },
            {
                "name": "show gamerule",
                "description": "",
                "commands": [
                    {
                        "type": "console",
                        "command": "gamerule",
                        "pattern": ""
                    }
                ],
                "subcommands": [
                    {
                        "name": ""
                    }
                ]
            },
            {
                "name": "set gamerule",
                "description": "",
                "commands": [
                    {
                        "type": "gamerule <text>",
                        "command": "",
                        "pattern": "^gamerule (?<name>[A-Za-z]+)$"
                    }
                ],
                "subcommands": [
                    {
                        "name": "name",
                        "pattern": ""
                    }
                ]
            },
            {
                "name": "template",
                "description": "",
                "commands": [
                    {
                        "type": "",
                        "command": "",
                        "pattern": ""
                    }
                ],
                "subcommands": [
                    {
                        "name": ""
                    }
                ]
            }
        ],
        "macro_commands": [
            {
                "name": "restart",
                "description": "",
                "commands": [
                    {
                        "type": "console",
                        "command": "say Restart the server after 3 minutes."
                    },
                    {
                        "type": "system",
                        "command": "wait 3 min"
                    },
                    {
                        "type": "console",
                        "command": "stop"
                    }
                ]
            }
        ]
    }
]
```
