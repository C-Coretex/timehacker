import React, { useState } from 'react';
import type { FC } from 'react';
import { Calendar, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { Typography, Button } from 'antd';
import { PlusOutlined, LoginOutlined } from '@ant-design/icons';
import api from '../api/api';

const { Title } = Typography;

// Initialize moment localizer for calendar
const localizer = momentLocalizer(moment);

// Sample events for testing (replace with API data later)
const initialEvents = [
    {
        id: '1',
        title: 'Sample Task',
        start: moment().toDate(),
        end: moment().add(1, 'hours').toDate(),
    },
    {
        id: '2',
        title: 'Meeting',
        start: moment().add(1, 'days').toDate(),
        end: moment().add(1, 'days').add(2, 'hours').toDate(),
    },
];

// Calendar page component to display tasks
const CalendarPage: FC = () => {
    const [events, setEvents] = useState(initialEvents);

    // Handle event selection (for future editing)
    const handleSelectEvent = (event: { id: string; title: string; start: Date; end: Date }) => {
        // TODO: Add logic to open TaskFormModal for editing
        console.log('Selected event:', event);
    };

    // Handle adding a new task (placeholder for modal)
    const handleAddTask = () => {
        // TODO: Open TaskFormModal and add new event to state
        console.log('Add new task');
    };

    const handleLogin = async () => {
        try {
            const response = await api.post(
                '/login?useCookies=true&useSessionCookies=true',
                {
                    email: 'arina@walrus.lv',
                    password: 'Separatum+45'
                }
            );
            console.log('Login success:', response.data);
        } catch (error: any) {
            console.error('Login error:', error);
        }
    };

    return (
        <div style={{ padding: '1rem' }}>
            <Title level={2}>Tasks Calendar</Title>
            <div style={{ marginBottom: '1rem', display: 'flex', gap: '8px' }}>
                <Button
                    type="primary"
                    icon={<LoginOutlined />}
                    onClick={handleLogin}
                >
                    Login
                </Button>

                <Button
                    type="dashed"
                    icon={<PlusOutlined />}
                    onClick={() => console.log('Add new task')}
                >
                    Add Task
                </Button>
            </div>
            <Calendar
                localizer={localizer}
                events={events}
                startAccessor="start"
                endAccessor="end"
                style={{ height: 500 }}
                onSelectEvent={handleSelectEvent}
            />
        </div>
    );
};

export default CalendarPage;