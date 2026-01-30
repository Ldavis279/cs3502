#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/wait.h>
int main() {

int pipefd[2];
pid_t pid;
char buffer[100];
char *message = "Hello from parent!";

// TODO: Create pipe using pipe(pipefd)
int result = pipe(pipefd);


// Check for errors (pipe returns -1 on failure)
if(pipe(pipefd) == -1){
perror("pipe failed");
return 1;
}

// TODO: Fork the process
pid = fork();
if (pid == 0) {
// Child process
// TODO: Close the write end (child only reads)
close(pipefd[1]);

// TODO: Read from pipe into buffer
ssize_t x = read(pipefd[0], buffer, sizeof(buffer) - 1);

// TODO: Print the received message
buffer[x] = '\0';
printf("Child received: %s\n", buffer);

// TODO: Close read end
close(pipefd[0]);
} 


else {
// Parent process
// TODO: Close the read end (parent only writes)
close(pipefd[0]);

// TODO: Write message to pipe
write(pipefd[1],message,strlen(message));

// TODO: Close write end
close(pipefd[1]);

// TODO: Wait for child to finish
wait(NULL);

}
return 0;
}

