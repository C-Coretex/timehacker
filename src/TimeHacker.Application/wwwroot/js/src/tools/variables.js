export let API_URL = ""
export function setApiUrl(newUrl) {
    API_URL = newUrl
}

export const POSSIBLE_COLORS_FOR_TASK = [
    '#FFCCE5',
    '#FC766AFF',
    '#5B84B1FF',
    '#5F4B8BFF',
    '#97BC62FF',
    '#0063B2FF',
    '#E94B3CFF',
    '#DAA03DFF',
    '#333D79FF',
    '#FFA177FF',
    '#A13941FF',
    '#964F4CFF',
    '#567572FF',
    '#8BBEE8FF',
    '#7DB46CFF',
    '#EEB238FF'
]

export const TEST_RESULT_FOR_DAY = {
    "tasksTimeline": [
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "00:00:00",
                "end": "00:40:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "00:45:00",
                "end": "01:55:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "02:00:00",
                "end": "02:26:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "02:31:00",
                "end": "03:41:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "03:46:00",
                "end": "04:26:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "04:31:00",
                "end": "05:41:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "05:46:00",
                "end": "06:26:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "06:31:00",
                "end": "07:06:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "07:11:00",
                "end": "08:21:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "08:26:00",
                "end": "08:46:00"
            }
        },
        {
            "isFixed": true,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task4",
                "description": "asdasd",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-09T17:47:16.9698901",
                "id": 4
            },
            "timeRange": {
                "start": "09:00:00",
                "end": "10:00:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "10:05:00",
                "end": "10:28:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "10:33:00",
                "end": "10:59:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "11:04:00",
                "end": "11:55:00"
            }
        },
        {
            "isFixed": true,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": "testdescr",
                "category": "a",
                "priority": 1,
                "isCompleted": false,
                "createdTimestamp": "2024-02-09T17:37:19.2303814",
                "id": 2
            },
            "timeRange": {
                "start": "12:00:00",
                "end": "13:00:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "13:05:00",
                "end": "13:49:00"
            }
        },

        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "14:25:00",
                "end": "14:55:00"
            }
        },
        {
            "isFixed": true,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": null,
                "category": "asdasd",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-09T17:37:40.2943042",
                "id": 3
            },
            "timeRange": {
                "start": "15:00:00",
                "end": "17:30:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "17:35:00",
                "end": "18:20:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "18:25:00",
                "end": "19:05:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "19:10:00",
                "end": "20:20:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "20:25:00",
                "end": "21:35:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task3",
                "description": "a",
                "category": "a",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-04T21:54:37.7891399",
                "id": 5
            },
            "timeRange": {
                "start": "21:40:00",
                "end": "22:20:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task1",
                "description": "test ddd",
                "category": "aaa",
                "priority": 2,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T13:54:11.6884736",
                "id": 1
            },
            "timeRange": {
                "start": "22:25:00",
                "end": "23:35:00"
            }
        },
        {
            "isFixed": false,
            "task": {
                "userId": "2d206f01-0d24-4640-8023-c0a0f03d20ca",
                "name": "task2",
                "description": null,
                "category": "b",
                "priority": 0,
                "isCompleted": false,
                "createdTimestamp": "2024-02-03T15:58:38.0725216",
                "id": 3
            },
            "timeRange": {
                "start": "23:40:00",
                "end": "23:59:59"
            }
        }
    ]
  }