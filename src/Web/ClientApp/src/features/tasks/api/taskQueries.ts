import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  createTask,
  createTaskStartPlan,
  getTaskDetails,
  getTasks,
} from './tasksApi';

const taskKeys = {
  all: ['tasks'] as const,
  detail: (taskId: number) => ['tasks', 'detail', taskId] as const,
};

export function useTasks() {
  return useQuery({
    queryKey: taskKeys.all,
    queryFn: getTasks,
  });
}

export function useCreateTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createTask,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: taskKeys.all }),
  });
}

export function useTaskDetails(taskId: number) {
  return useQuery({
    queryKey: taskKeys.detail(taskId),
    queryFn: () => getTaskDetails(taskId),
    enabled: Number.isInteger(taskId) && taskId > 0,
  });
}

export function useCreateTaskStartPlan(taskId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (language: 'tr' | 'en') => createTaskStartPlan(taskId, language),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: taskKeys.detail(taskId) }),
  });
}
