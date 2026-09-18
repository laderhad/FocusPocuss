import {
  CreateTaskStartPlanRequest,
  CreateTaskCommand,
  TasksClient,
  type TaskDto,
  type TaskDetailsDto,
  type TaskStartPlanDto,
} from '../../../web-api-client';

const tasksClient = new TasksClient();

export function getTasks(): Promise<TaskDto[]> {
  return tasksClient.getTasks();
}

export function createTask(originalInput: string): Promise<TaskDto> {
  return tasksClient.createTask(new CreateTaskCommand({ originalInput }));
}

export function getTaskDetails(taskId: number): Promise<TaskDetailsDto> {
  return tasksClient.getTaskDetails(taskId);
}

export function createTaskStartPlan(
  taskId: number,
  language: 'tr' | 'en',
): Promise<TaskStartPlanDto> {
  return tasksClient.createTaskStartPlan(
    taskId,
    new CreateTaskStartPlanRequest({ language }),
  );
}

export type { TaskDetailsDto, TaskDto, TaskStartPlanDto };
