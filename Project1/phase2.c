#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <time.h>
#include <errno.h>
#include <string.h>



// Configuration - experiment with different values !
# define NUM_ACCOUNTS 2
# define NUM_THREADS 4
# define TRANSACTIONS_PER_THREAD 10
# define INITIAL_BALANCE 1000.0


// Updated Account structure with mutex ( GIVEN )
typedef struct {
int account_id ;
double balance ;
int transaction_count ;
pthread_mutex_t lock ; // NEW: Mutex for this account
} Account ;


// Global shared array - THIS CAUSES RACE CONDITIONS !
Account accounts [ NUM_ACCOUNTS ];


//Tracks changes to verify 
//Will see  what changes happened
 static double total_deposited = 0.0;
 static double total_withdrawn = 0.0;


// GIVEN : Example of mutex initialization
void initialize_accounts () {
    for (int i = 0; i < NUM_ACCOUNTS ; i ++) {
    accounts[i].account_id = i ;
    accounts[i].balance = INITIAL_BALANCE;
    accounts[i].transaction_count = 0;
    // Initialize the mutex
    pthread_mutex_init(&accounts[i].lock , NULL ) ;
    }
}



// GIVEN : Example deposit function WITH proper protection
void deposit_safe (int account_id , double amount ) {

//Tracks deposited for checking    
total_deposited += amount;

// Acquire lock BEFORE accessing shared data
pthread_mutex_lock (&accounts[account_id].lock ) ;

// ===== CRITICAL SECTION =====
// Only ONE thread can execute this at a time for this account
accounts [ account_id ]. balance += amount ;
accounts [ account_id ]. transaction_count ++;
// ============================
// Release lock AFTER modifying shared data
pthread_mutex_unlock (&accounts[account_id].lock);
}



// TODO 1: Implement withdrawal_safe () with mutex protection
// Reference : Follow the pattern of deposit_safe () above
// Remember : lock BEFORE accessing data , unlock AFTER
void withdrawal_safe (int account_id , double amount ) {
// YOUR CODE HERE
// Hint : pthread_mutex_lock
// Hint : Modify balance
// Hint : pthread_mutex_unlock

//Tracks withdrawn for checking    
total_withdrawn += amount;

pthread_mutex_lock (&accounts[account_id].lock ) ;

double current_balance = accounts[account_id].balance ;
double new_balance = current_balance;

    new_balance = current_balance - amount ;
    accounts[account_id].balance = new_balance ;
    accounts[account_id].transaction_count++;


pthread_mutex_unlock (&accounts[account_id].lock);
}




// TODO 2: Update teller_thread to use safe functions
// Change : deposit_unsafe -> deposit_safe
// Change : withdrawal_unsafe -> withdrawal_safe
void * teller_thread ( void * arg ) {

int teller_id = *(int*) arg ; // GIVEN : Extract thread ID
unsigned int seed = time(NULL) ^ pthread_self();

for (int i = 0; i < TRANSACTIONS_PER_THREAD ; i ++) {

int account_idx = rand_r(&seed) % NUM_ACCOUNTS;
double amount = (rand_r(&seed)%100 + 1);
int operation =  rand_r(&seed) % 2;


if ( operation == 1) {
deposit_safe( account_idx , amount ) ;
printf (" Teller %d: Deposited $%.2f to Account %d\n",
teller_id , amount , account_idx ) ;

} else {
    withdrawal_safe(account_idx, amount);
    printf (" Teller %d: Withdrawled $%.2f to Account %d\n",
    teller_id , amount , account_idx ) ;
}
}
return NULL ;
}




// TODO 3: Add performance timing
// Reference : Section 7.2 " Performance Measurement "
// Hint : Use clock_gettime ( CLOCK_MONOTONIC , & start );

int main () {


  total_deposited = 0.0;
  total_withdrawn = 0.0;


initialize_accounts();
struct timespec start , end ;
clock_gettime (CLOCK_MONOTONIC, &start) ;   

printf ("=== Phase 2: Race Conditions Solutions Demo ===\n\n") ;



// Display initial state ( GIVEN )
printf(" Initial State :\n");
for (int i = 0; i < NUM_ACCOUNTS ; i ++) {
printf (" Account %d: $%.2f\n", i , accounts [ i ]. balance ) ;
}


double expected_total = NUM_ACCOUNTS * INITIAL_BALANCE;
printf ("\nExpected total : $%.2f\n\n", expected_total ) ;



pthread_t threads [ NUM_THREADS ];
int thread_ids [ NUM_THREADS ]; // GIVEN : Separate array for IDs



for (int i = 0; i < NUM_THREADS ; i ++) {
thread_ids[i] = i; // GIVEN : Store ID persistently
pthread_create (&threads[i], NULL, teller_thread, &thread_ids[i]);
}




for (int i = 0; i < NUM_THREADS ; i ++) {
    pthread_join(threads[i],NULL);
}


//gets expected total
expected_total = expected_total + total_deposited - total_withdrawn;

printf ("\n=== Final Results ===\n") ;
double actual_total = 0.0;
for (int i = 0; i < NUM_ACCOUNTS ; i ++) {
printf (" Account %d: $%.2f (%d transactions )\n",
i , accounts [ i ]. balance , accounts [ i ]. transaction_count ) ;
actual_total += accounts [ i ]. balance ;
}
printf ("\nExpected total : $%.2f\n", expected_total ) ;
printf (" Actual total : $%.2f\n", actual_total ) ;
printf (" Difference : $%.2f\n", actual_total - expected_total ) ;

printf("\n\n");

if(actual_total != expected_total)
{
    printf("RACE CONDITION DETECTED !");
}
else{
     printf("NO RACE CONDITION DETECTED!\nGOOD JOB!");
}



clock_gettime (CLOCK_MONOTONIC , &end ) ;
double elapsed = (end.tv_sec - start.tv_sec ) +
(end.tv_nsec - start.tv_nsec) / 1e9 ;

printf ("\n\nTime : %.4f seconds \n", elapsed ) ;
printf("\n\n");
return 0;
}
