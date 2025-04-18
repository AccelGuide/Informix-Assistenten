#!/bin/bash
# Erstelle einen täglichen Report für den Informix-Server
REPORT_FILE="/var/log/informix/daily_report_$(date +%Y%m%d).log"
echo "Informix System Report - $(date)" > ${REPORT_FILE}
onstat -g ses >> ${REPORT_FILE}
onstat -V >> ${REPORT_FILE}
mail -s "Daily Informix Report" admin@example.com < ${REPORT_FILE}