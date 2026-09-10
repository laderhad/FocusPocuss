import {
  CreateTaskCommand,
  TasksClient,
  type TaskDto,
} from '../../../web-api-client';

const tasksClient = new TasksClient();

export function getTasks(): Promise<TaskDto[]> {
  return tasksClient.getTasks();
}

export function createTask(originalInput: string): Promise<TaskDto> {
  return tasksClient.createTask(new CreateTaskCommand({ originalInput }));
}

export type { TaskDto };
