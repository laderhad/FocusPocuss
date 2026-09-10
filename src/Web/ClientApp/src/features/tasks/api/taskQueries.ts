import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createTask, getTasks } from './tasksApi';

const taskKeys = {
  all: ['tasks'] as const,
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
