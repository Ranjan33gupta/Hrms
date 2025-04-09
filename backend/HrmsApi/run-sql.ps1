$env:PGPASSWORD = "postgres"
psql -U postgres -d hrms_v2 -f apply-chatbot-tables.sql
