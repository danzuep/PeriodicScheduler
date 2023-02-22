#cd C:\Source\danzuep\PeriodicScheduler\
docker rmi scheduler-image; docker build -t scheduler-image .; docker run --rm -itp 6080:80 --name scheduler-container scheduler-image
