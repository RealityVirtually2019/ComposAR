import requests
import gevent
import random
import string
import main
import os

from flask import Flask, request
app = Flask(__name__)

return_val = 'no'

@app.route('/submit', methods=['POST'])
def doEverything():
  print(request.files)
  for key, file  in request.files.items():
    file.save('./../../../Data/CharacterDraw/sketch/m1/' + file.filename)
  # main.main()  
  global return_val
  gevent.joinall([gevent.spawn(spawned)])
  return return_val
  
def spawned():
  global return_val
  print('yoting')
  payload = {'client_id': os.environ['FORGE_CLIENT_ID'], \
            'client_secret': os.environ['FORGE_CLIENT_SECRET'], \
            'grant_type': 'client_credentials', \
            'scope': 'data:write'}
  resp = requests.post('https://developer.api.autodesk.com/authentication/v1/authenticate', \
                headers={'Content-Type': 'application/x-www-form-urlencoded'}, \
                data=payload)
  access_token = resp.json()['access_token']
  
  print("token: " + access_token)

  name = ''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(5))
  payload2 = {'scenename': name, 'format': 'rcm'}
  resp2 = requests.post('https://developer.api.autodesk.com/photo-to-3d/v1/photoscene', \
                headers={'content-type': 'application/json', 'Authorization': 'Bearer ' + access_token}, \
                data=payload2)
  # print("error, if there was one otherwise None: " + resp2.json()['Error'])
  
  print(resp2.json())
  print(resp2.json()['Photoscene']['photosceneid'])
  return_val = resp2.json()['Photoscene']['photosceneid']


  headers = {\
      'Authorization': 'Bearer ' + access_token,\
  }

  files = {\
      'photosceneid': (None, 'hcYJcrnHUsNSPII9glhVe8lRF6lFXs4NHzGqJ3zdWMU\n'),\
      'type': (None, 'image\n'),\
      'file[0]': ('./../../../Data/CharacterDraw/sketch/sketch-F-0.png', open('./../../../Data/CharacterDraw/sketch/sketch-F-0.png', 'rb')),\
      'file[1]': ('./../../../Data/CharacterDraw/sketch/sketch-S-0.png', open('./../../../Data/CharacterDraw/sketch/sketch-S-0.png', 'rb')),\
  }
  response = requests.post('https://developer.api.autodesk.com/photo-to-3d/v1/file', headers=headers, files=files)
  print(response)

  # payload3 = {'photosceneid': sceneId, 'type': 'image', \
  #             'file[0]':}
  # resp3 = requests.post('https://developer.api.autodesk.com/photo-to-3d/v1/file', \
  #               headers={'content-type': 'application/json', 'Authorization': 'Bearer ' + access_token}, \
  #               params=payload3)
  
if __name__ == '__main__':
    app.run()