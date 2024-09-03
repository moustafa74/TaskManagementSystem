$(document).ready(function () {
    // URL to fetch tasks data
    const tasksApiUrl = '/api/Task';

    // Fetch data from API and update the dashboard
    $.ajax({
        url: tasksApiUrl,
        method: 'GET',
        success: function (data) {
            // Update the statistics
            updateStatistics(data);

            // Generate chart data and render the chart
            generateChart(data);
        },
        error: function (error) {
            console.error('Error fetching tasks:', error);
            alert('An error occurred while fetching tasks. Please try again later.');
        }
    });

    // Function to update statistics
    function updateStatistics(tasks) {
        const totalTasks = tasks.length;
        const completedTasks = tasks.filter(task => task.status === 'Completed').length;
        const inProgressTasks = tasks.filter(task => task.status === 'In Progress').length;
        const overdueTasks = tasks.filter(task => moment(task.dueDate).isBefore(moment()) && task.status !== 'Completed').length;

        $('#totalTasks').text(totalTasks);
        $('#completedTasks').text(completedTasks);
        $('#inProgressTasks').text(inProgressTasks);
        $('#overdueTasks').text(overdueTasks);
    }

    // Function to generate chart data and render the chart
    function generateChart(tasks) {
        const labels = ['Low', 'Medium', 'High', 'Critical'];
        const prioritiesCount = [0, 0, 0, 0];

        tasks.forEach(task => {
            switch (task.priority) {
                case 'Low':
                    prioritiesCount[0]++;
                    break;
                case 'Medium':
                    prioritiesCount[1]++;
                    break;
                case 'High':
                    prioritiesCount[2]++;
                    break;
                case 'Critical':
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
