CREATE ROLE application_user WITH LOGIN PASSWORD 'application_password';

GRANT CONNECT ON DATABASE "TimeHacker" TO application_user;
GRANT USAGE ON SCHEMA public TO application_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO application_user;

-- Also grant on future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public 
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO application_user;