#!/bin/bash
# Kontrollierter Neustart des Informix-Servers

echo "Beende Informix-Server kontrolliert..."
onmode -ky

echo "Starte Informix-Server neu..."
oninit -vy

echo "Neustart abgeschlossen."