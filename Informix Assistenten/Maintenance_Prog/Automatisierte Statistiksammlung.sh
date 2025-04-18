#!/bin/bash
# Automatisierte Statistiksammlung: Erstelle ein Skript, das regelmäßig (z. B. täglich oder wöchentlich) die Statistiken abruft und abspeichert:

STATS_DIR="/var/log/informix"
STATS_FILE="${STATS_DIR}/stats_$(date +%Y%m%d).log"
onstat -g stats > ${STATS_FILE}