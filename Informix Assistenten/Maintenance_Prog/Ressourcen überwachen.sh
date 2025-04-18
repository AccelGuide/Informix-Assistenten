#!/bin/bash
# Suche nach dem Informix-Prozess und zeige CPU- und Speicherverbrauch an
ps aux | grep oninit | grep -v grep