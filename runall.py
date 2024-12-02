import os
import time

# Ruta de los dos códigos Python que deseas ejecutar
ruta_codigo1 = "versionsuperoptimizada.py"
ruta_codigo2 = "main_EEG_Trigger_saver_EEG.py"

# Abrir código 1 en una nueva terminal (para Windows)
os.system(f"start cmd /c python {ruta_codigo1}")

# Esperar 2 segundos
time.sleep(2)

# Abrir código 2 en una nueva terminal (para Windows)
os.system(f"start cmd /c python {ruta_codigo2}")
