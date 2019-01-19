FROM python:3.5.6-slim

# Copy files over
WORKDIR /CNNServer
COPY ./ /CNNServer

ENV PATH=/home/ubuntu/.virtualenvs/bin:$PATH

# Install deps with pip
RUN pip3 install tensorflow
RUN pip3 install flask
RUN pip3 install scipy
RUN pip3 install requests
RUN pip3 install gevent

# Extenal port to expose
EXPOSE 5000

# Env vars
ENV FLASK_APP server.py
# pls dont steal dis
ENV FORGE_CLIENT_ID p7EoOa2TjOdSJwNTwF1kstzkjaqlKYn2
ENV FORGE_CLIENT_SECRET 6CKbUmJ2x8l93g7q

CMD cd Network/code/MonsterNet && flask run
