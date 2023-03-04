# AlgoStore

## Prerequisite
1. Install MongoDB
2. Install Visual Studio 2022 (with .net 6.0)

## Steps to run
1. Build and run all 3 projects
2. Follow the steps on the GrpcClient console app for the first time
3. Run the GrpcClient again for the second time

## Client Logic
1. Add mocked data into MongoDB
2. See the added data
3. Reserve 3 products for product[0], product[1] and product[2]
4. An order is created
5. Run the console app again, repeat the steps, but this time stocks are not reserved because the quantity is insufficient
