#include <stdio.h>
#include <unistd.h>

int main() {

int pid;
int pipefd1[2], pipefd2[2];
int result1,result2;

char parentMessage[20] = "Hello child!";
char childMessage[20] = "Hi parent!";
char readMessage[20];

result1 = pipe(pipefd1);
if(result1 == -1){
    printf("Failed to create pipe1");
}

result2 = pipe(pipefd2);
if(result2 == -1) {
    printf("Failed to create pipe1");
}

pid = fork();

if(pid == 0){
//child process
close(pipefd1[1]);
close(pipefd2[0]);

read(pipefd1[0],readMessage,sizeof(readMessage));
printf("Parent says to child: %s\n", readMessage);

write(pipefd2[1], childMessage, sizeof(childMessage));

}

 else{
    //parent process
    close(pipefd1[0]);
    close(pipefd2[1]);

write(pipefd1[1], parentMessage, sizeof(parentMessage));


read(pipefd2[0],readMessage,sizeof(readMessage));
printf("Child says to parent: %s\n", readMessage);



}
return 0;
}
