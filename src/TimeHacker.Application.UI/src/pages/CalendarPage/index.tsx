import React, { useState, useEffect, useCallback } from 'react';
import type { FC } from 'react';
import { Calendar, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { Typography, Button, Spin, Alert, Modal } from 'antd';
import { PlusOutlined, ReloadOutlined } from '@ant-design/icons';
import api from '../../api/api';

const { Title } = Typography;

const localizer = momentLocalizer(moment);

const CalendarPage: FC = () => {
  const [events, setEvents] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedTask, setSelectedTask] = useState<any | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);

  const handleSelectEvent = (event: any) => {
    setSelectedTask(event);
    setIsModalVisible(true);
  };

  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.get('/api/FixedTasks/GetAll');
      const tasks = response.data.map((task: any) => ({
        id: task.id,
        title: task.name,
        start: new Date(task.startTimestamp),
        end: new Date(task.endTimestamp),
        allDay: false,
        description: task.description,
      }));
      setEvents(tasks);
    } catch (err: any) {
      console.error('Failed to fetch tasks:', err);
      setError(err.response?.data?.message || 'Failed to load tasks');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  return (
    <div style={{ padding: '1rem' }}>
      <Title level={2}>Tasks Calendar</Title>

      <div style={{ marginBottom: '1rem', display: 'flex', gap: '8px' }}>
        <Button icon={<ReloadOutlined />} onClick={fetchTasks}>
          Refresh
        </Button>
      </div>

      {error && <Alert type="error" message={error} showIcon style={{ marginBottom: '1rem' }} />}
      {loading ? (
        <Spin size="large" style={{ display: 'block', margin: '2rem auto' }} />
      ) : (
        <Calendar
          localizer={localizer}
          events={events}
          startAccessor="start"
          endAccessor="end"
          style={{ height: 500 }}
          onSelectEvent={handleSelectEvent}
        />
      )}

      <Modal
        open={isModalVisible}
        title="Task Details"
        footer={[
          <Button key="close" onClick={() => setIsModalVisible(false)}>
            Close
          </Button>,
        ]}
        onCancel={() => setIsModalVisible(false)}
      >
        {selectedTask && (
          <div>
            <p><b>ID:</b> {selectedTask.id}</p>
            <p><b>Title:</b> {selectedTask.title}</p>
            <p><b>Description:</b> {selectedTask.description || '-'}</p>
            <p><b>Start:</b> {moment(selectedTask.start).format('YYYY-MM-DD HH:mm')}</p>
            <p><b>End:</b> {moment(selectedTask.end).format('YYYY-MM-DD HH:mm')}</p>
          </div>
        )}
      </Modal>

    </div>
  );
};

export default CalendarPage;
