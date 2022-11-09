import requests
import json
import streamlit as st

import matplotlib.pyplot as plt
from scipy.misc import electrocardiogram
from scipy.signal import find_peaks
import pandas as pd
pd.options.display.float_format = '{:,.10f}'.format
import numpy as np
import json
from sklearn import preprocessing
# from urllib.request
data = requests.get('http://localhost:5000').json()
#print(res_a)
# f = open("demo/get_response.json")
# data = json.load(f)
#print(data)
vel = []
time = []
diff_vel = []
diff_time = []
vel0 = 0
vel1 = 0
time0 = 0
time1 = 0
for i in data:
    vel0 = float(vel1)
    #vel0 = float(vel1) * 5 / 18
    line = str(i)
    vel1 = float(line.split(": ")[1].split(",")[0])
    #vel1 = float(line.split(": ")[1].split(",")[0]) * 5 / 18
    diff_vel_temp = float(vel1 - vel0)
    
    time0 = int(time1)
    time1 = int(line.split(": ")[2].split("}")[0])
    diff_time_temp = float(time1 - time0)
    
    #diff_vel.append(diff_vel_temp)
    #diff_time.append(diff_time_temp)
    vel.append(vel1)
    time.append(time1)
    #print(line.split(": ")[1].split(",")[0] + " " + line.split(": ")[2].split("}")[0])

preprocessing.normalize([vel])
preprocessing.normalize([time])

for i in range(len(vel)):
    if i == 0:
        diff_vel.append(0)
    else:
        diff_vel.append(vel[i] - vel[i - 1])

for i in range(len(time)):
    if i == 0:
        diff_time.append(0)
    else:
        diff_time.append(time[i] - time[i - 1])

acc = []
for i in range(len(vel)):
    if i == 0 :
        acc.append(0)
    else:
        acc.append(diff_vel[i] * 1000 / (diff_time[i]))

acc_diff = []
for i in range(len(vel)):
    if i == 0:
        acc_diff.append(0)
    else:
        acc_diff.append(acc[i] - acc[i - 1])

jerk = []
for i in range(len(vel)):
    if i == 0 :
        jerk.append(0)
    else:
        jerk.append(abs(acc_diff[i] * 1000 / (diff_time[i])))

data_list = [vel, time, diff_vel, diff_time, acc, acc_diff, jerk]
#i = 0
#while i < len(vel):
#    print[i]
#    i += 1
df_table = pd.DataFrame(data_list).transpose()
df_table.columns = ['velocity', 'time', 'diff_vel', 'diff_time', 'acc', 'acc_diff', 'jerk']
df_table['peak'] = np.where(df_table['jerk'] >= 0.6 , 0, 1)
overall_score = df_table['peak'].sum() / df_table['velocity'].count()
overall_time = df_table['diff_time'].sum() 
print('Your score for all trip = ', overall_score * 100)
last_100_score = df_table['peak'].tail(100).sum() / df_table['peak'].tail(100).count()
last_100_time = df_table['diff_time'].tail(100).sum() 
print('Your score for last ', last_100_time / 1000, 'seconds = ', last_100_score * 100)
sidebar_text = f"# Introduction\n\nWe would like to provide realtime feedback to the drivers: \n\n - [**Muto**](https://dashboard.composiv.ai/): Choose the vehicle\n- [**Vehicle**](https://github.com/eclipse-sdv-hackathon-bcx): Foxglove Playground\n- [**Studio Foxglove**](https://studio.foxglove.dev/)"
x = df_table['jerk']
peaks, _ = find_peaks(x, distance=1)
np.diff(peaks)
plt.plot(x)
plt.plot(peaks, x[peaks], "x")
a = plt.show()

# config streamlit
st.set_page_config(layout="wide")
st.markdown(""" <style>
#MainMenu {visibility: hidden;}
footer {visibility: hidden;}
</style> """, unsafe_allow_html=True)

padding = 0.5
st.markdown(f""" <style>
    .reportview-container .main .block-container{{
        padding-top: {padding}rem;
        padding-right: {padding}rem;
        padding-left: {padding}rem;
        padding-bottom: {padding}rem;
    }} </style> """, unsafe_allow_html=True)

c1, c2 = st.columns(2)

st.sidebar.markdown(sidebar_text)
x = requests.get('http://localhost:5000')

print(x.status_code)
print(x.json())
res_json = x.json()

# st.title('Driving Score in Real Time')

with c1:
    st.set_option('deprecation.showPyplotGlobalUse', False)
    st.pyplot(a)

    st.markdown("#### Driver Score")
    st.markdown(f"**Score:** {peaks}")
    st.markdown(f"**Score_overall:** {overall_score*100}")
    st.markdown(f"**Score_Last_10s:** {last_100_score*100}")




# with open("vehice.json", 'w') as f:
#     json.dump(res_json, f)
video_file = open('demo/f1_racer_without_events.mp4', 'rb')
video_bytes = video_file.read()
with c2:
    st.video(video_bytes)