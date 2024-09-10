$(document).ready(function () {
    function isTokenExpired(token) {
        if (!token) return true;

        // Decode the token payload 
        const payload = JSON.parse(atob(token.split('.')[1]));
        console.log(payload);
        const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
        console.log(currentTime);
        return payload.exp < currentTime;
    }
    if (isTokenExpired(localStorage.getItem('authToken'))) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('UserID');
    }
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = 'index.html';
    }
    const userId = localStorage.getItem('UserID');
    // Fetch data from API and update the dashboard
    $.ajax({
        url: `/api/Task/user/${userId}`,
        method: 'GET',
        headers: {
            Authorization: `Bearer ${token}`,
        },
        success: function (data) {
            console.log(data); 
            updateStatistics(data);
            generateChart(data);
        },
        error: function (error) {
            console.error('Error fetching dashboard data:', error);
        }
    });

    // Function to update statistics
    function updateStatistics(tasks) {
        const totalTasks = tasks.length;
        const completedTasks = tasks.filter(task => task.status === 2).length;
        const inProgressTasks = tasks.filter(task => task.status === 1).length;
        const overdueTasks = tasks.filter(task => moment(task.dueDate).isBefore(moment()) && task.status !== 2).length;
        const todoTasks = tasks.filter(task => task.status === 0).length;

        $('#totalTasks').text(totalTasks);
        $('#completedTasks').text(completedTasks);
        $('#inProgressTasks').text(inProgressTasks);
        $('#overdueTasks').text(overdueTasks);
        $('#ToDOTasks').text(todoTasks); 
    }

    // Function to generate chart data and render the chart
    function generateChart(tasks) {
        const labels = ['Low', 'Medium', 'High', 'Critical'];
        const prioritiesCount = [0, 0, 0, 0];

        tasks.forEach(task => {
            switch (task.priority) {
                case 0:
                    prioritiesCount[0]++;
                    break;
                case 1:
                    prioritiesCount[1]++;
                    break;
                case 2:
                    prioritiesCount[2]++;
                    break;
                case 3:
                    prioritiesCount[3]++;
                    break;
            }
        });

        const ctx = $('#tasksChart');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: '# of Tasks',
                    data: prioritiesCount,
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(75, 192, 192, 0.2)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(255, 99, 132, 1)',
                        'rgba(75, 192, 192, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
});
