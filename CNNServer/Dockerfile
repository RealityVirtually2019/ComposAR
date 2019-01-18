FROM python:3.5.6-slim

# Copy files over
WORKDIR /CNNServer
COPY ./ /CNNServer

# Install deps with pip
RUN pip3 install tensorflow
RUN pip3 install scipy

# Extenal port to expose
EXPOSE 3000

# Env vars
ENV FORGE_CLIENT_ID p7EoOa2TjOdSJwNTwF1kstzkjaqlKYn2
ENV FORGE_CLIENT_SECRET 6CKbUmJ2x8l93g7q

# Run start cmd here
CMD ["ls"]
